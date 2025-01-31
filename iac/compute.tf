resource "tls_private_key" "ssh" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "local_sensitive_file" "pem_file" {
  filename             = pathexpand("./${local.app_name}-yc.pem")
  file_permission      = "600"
  directory_permission = "700"
  content              = tls_private_key.ssh.private_key_pem
}

resource "yandex_iam_service_account" "compute_sa" {
  folder_id = local.folder_id
  name      = "${local.app_name}-compute-sa"
}


resource "yandex_container_repository_iam_binding" "signature_api_puller" {
  repository_id = data.yandex_container_repository.i_bashmaky.id
  role          = "container-registry.images.puller"

  members = [
    "serviceAccount:${yandex_iam_service_account.compute_sa.id}",
  ]
}

locals {
  server_compose = {
    container_name = "server"
    image          = "cr.yandex/${data.yandex_container_repository.i_bashmaky.name}:${local.api_version}"
    ports = [
      "80:80",
    ]
    logging = {
      driver = "fluentd"
      options = {
        # Fluent Bit слушает логи на порту 24224.
        fluentd-address = "localhost:24224"
        # Теги используются для маршрутизации логов.
        tag = "server.logs"
      }
    }
    restart = "always"
    environment = {
      ConnectionStrings__PostgresConnection = "Server=${yandex_mdb_postgresql_cluster.database.host[0].fqdn};Port=6432;Database=${yandex_mdb_postgresql_database.db.name};User ID=${yandex_mdb_postgresql_user.db_user.name};Password=${yandex_mdb_postgresql_user.db_user.password};Encoding=UTF8;Client Encoding=UTF8;",
      CommonSettings__BaseUrl               = "https://${local.host_name}/",
    }
  }


  compose = {
    version = "3.7"
    services = {
      server    = local.server_compose
      fluentbit = local.fluentbit_compose
    }
  }

  cloud_config = {
    write_files = local.fluentbit_cloud_config_files
  }
}


data "yandex_compute_image" "container-optimized-image" {
  family = "container-optimized-image"
}

resource "yandex_resourcemanager_folder_iam_member" "vpc_admin" {
  folder_id = data.yandex_resourcemanager_folder.current.folder_id

  role   = "vpc.admin"
  member = "serviceAccount:${yandex_iam_service_account.compute_sa.id}"
}
resource "yandex_resourcemanager_folder_iam_member" "vpc_user" {
  folder_id = data.yandex_resourcemanager_folder.current.folder_id

  role   = "vpc.user"
  member = "serviceAccount:${yandex_iam_service_account.compute_sa.id}"
}
resource "yandex_resourcemanager_folder_iam_member" "editor" {
  folder_id = data.yandex_resourcemanager_folder.current.folder_id

  role   = "editor"
  member = "serviceAccount:${yandex_iam_service_account.compute_sa.id}"
}
resource "yandex_compute_instance_group" "default" {
  name               = "${local.app_name}-compute"
  service_account_id = yandex_iam_service_account.compute_sa.id
  instance_template {
    service_account_id = yandex_iam_service_account.compute_sa.id
    platform_id        = "standard-v1"
    resources {
      memory = 2
      cores  = 2
    }
    boot_disk {
      mode = "READ_WRITE"
      initialize_params {
        image_id = data.yandex_compute_image.container-optimized-image.id
      }
    }
    network_interface {
      network_id = data.terraform_remote_state.vpc.outputs.network_id
      subnet_ids = [data.terraform_remote_state.vpc.outputs.subnets.a.id]
      nat        = true
    }
    metadata = {
      enable-oslogin = true
      # install-unified-agent = 1

      docker-compose = yamlencode(local.compose)
      # docker-container-declaration = yamlencode(local.coi)
      user-data          = "#cloud-config\n${yamlencode(local.cloud_config)}"
#      ssh-keys           = "normal:${tls_private_key.ssh.public_key_openssh}"
      serial-port-enable = 1
    }
  }
  application_load_balancer {
    target_group_name = "${local.app_name}-compute-target-group"
  }
  scale_policy {
    fixed_scale {
      size = 1
    }
  }
  allocation_policy {
    zones = ["ru-central1-a"]
  }
  deploy_policy {
    max_unavailable = 2
    max_creating    = 2
    max_expansion   = 2
    max_deleting    = 2
  }

  depends_on = [yandex_resourcemanager_folder_iam_member.vpc_admin]
}
