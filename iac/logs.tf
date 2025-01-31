resource "yandex_logging_group" "logs" {
  name             = "${local.app_name}"
  retention_period = "24h"
}

resource "yandex_resourcemanager_folder_iam_member" "log-writer" {
  folder_id = data.yandex_resourcemanager_folder.current.folder_id

  role   = "logging.writer"
  member = "serviceAccount:${yandex_iam_service_account.compute_sa.id}"
}
