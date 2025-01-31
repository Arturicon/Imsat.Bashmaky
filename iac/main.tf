terraform {
  required_providers {
    yandex = {
      source = "yandex-cloud/yandex"
    }
  }
  required_version = ">= 0.13"
  backend "s3" {

    endpoints = { s3 = "https://storage.yandexcloud.net" }
    region    = "ru-central1"

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true

    bucket   = "terraform-8b"
    key      = "i-bashmaky.tfstate"
  }
}

variable "app_version" {
  default = "latest"
}


locals {
  folder_id   = "b1g0jr81peqkostjgikq"
  api_version = var.app_version
  app_name = "i-bashmaky"
  host_name = "i-bashmaky.8bridge.org"
}

provider "yandex" {
  zone                     = "ru-central1-a"
  service_account_key_file = "key.json"
  folder_id                = local.folder_id
}


data "yandex_container_repository" "i_bashmaky" {
  repository_id = "crpevtvmsjt3d90fqb2g"
}

resource "random_password" "database_password" {
  length           = 16
  special          = true
  override_special = "_%@"
}


data "yandex_resourcemanager_folder" "current" {
  folder_id = "${local.folder_id}"
}

