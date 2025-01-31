data "terraform_remote_state" "dns" {
  backend = "s3"
  config = {
    endpoints = { s3 = "https://storage.yandexcloud.net" }
    region    = "ru-central1"

    access_key = var.backend_access_key
    secret_key = var.backend_secret_key

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true

    bucket   = "terraform-8b"
    key      = "dns.tfstate"
  }
}

data "terraform_remote_state" "vpc" {
  backend = "s3"
  config = {
    endpoints = { s3 = "https://storage.yandexcloud.net" }
    region    = "ru-central1"

    access_key = var.backend_access_key
    secret_key = var.backend_secret_key

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true

    bucket   = "terraform-8b"
    key      = "default-vnet.tfstate"
  }
}

data "terraform_remote_state" "registry" {
  backend = "s3"
  config = {
    endpoints = { s3 = "https://storage.yandexcloud.net" }
    region    = "ru-central1"

    access_key = var.backend_access_key
    secret_key = var.backend_secret_key

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true

    bucket   = "terraform-8b"
    key      = "container-repo.tfstate"
  }
}

data "terraform_remote_state" "common" {
  backend = "s3"
  config = {
    endpoints = { s3 = "https://storage.yandexcloud.net" }
    region    = "ru-central1"

    access_key = var.backend_access_key
    secret_key = var.backend_secret_key

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true

    bucket   = "terraform-8b"
    key      = "common.tfstate"
  }
}