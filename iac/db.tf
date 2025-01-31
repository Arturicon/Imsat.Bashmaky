resource "yandex_vpc_security_group" "database" {
  name       = "${local.app_name}_database_sg"
  network_id = data.terraform_remote_state.vpc.outputs.network_id

  ingress {
    description    = "PostgreSQL"
    port           = 6432
    protocol       = "TCP"
    v4_cidr_blocks = data.terraform_remote_state.vpc.outputs.subnets.a.cdir
  }
}


resource "yandex_mdb_postgresql_cluster" "database" {
  name                = "${local.app_name}_pg"
  environment         = "PRODUCTION"
  network_id          = data.terraform_remote_state.vpc.outputs.network_id
  security_group_ids  = [yandex_vpc_security_group.database.id]
  deletion_protection = true

  config {
    version = 15
    resources {
      resource_preset_id = "s2.micro"
      disk_type_id       = "network-ssd"
      disk_size          = "20"
    }
    access {
      web_sql = true
    }
  }



  host {
    zone      = "ru-central1-a"
    name      = "${local.app_name}-pg-host-a"
    subnet_id = data.terraform_remote_state.vpc.outputs.subnets.a.id
  }

  labels = {
    app = "${local.app_name}"
  }
}

resource "yandex_mdb_postgresql_user" "db_user" {
  cluster_id = yandex_mdb_postgresql_cluster.database.id
  name       = "${local.app_name}"
  password   = random_password.database_password.result
}

resource "yandex_mdb_postgresql_database" "db" {
  cluster_id = yandex_mdb_postgresql_cluster.database.id
  name       = "${local.app_name}_new"
  owner      = yandex_mdb_postgresql_user.db_user.name
  lc_collate = "en_US.UTF-8"
  lc_type    = "en_US.UTF-8"
}
