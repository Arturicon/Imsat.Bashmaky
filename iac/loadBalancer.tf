
resource "yandex_alb_backend_group" "server-backend-group" {
  name = "${local.app_name}-server-backend-group"

  http_backend {
    name             = "${local.app_name}-server-http-backend"
    weight           = 1
    port             = 80
    target_group_ids = ["${yandex_compute_instance_group.default.application_load_balancer.0.target_group_id}"]
    load_balancing_config {
      panic_threshold = 50
    }
    healthcheck {
      timeout  = "1s"
      interval = "1s"
      http_healthcheck {
        path = "/health"
      }
    }
  }
}

resource "yandex_alb_virtual_host" "vhost" {
  name           = "${local.app_name}-virtual-host"
  http_router_id = data.terraform_remote_state.common.outputs.router_id
  authority      = [local.host_name]
  route {
    name = "server-route"
    http_route {
      http_match {
        path {
          prefix = "/"
        }
      }
      http_route_action {
        backend_group_id = yandex_alb_backend_group.server-backend-group.id
        timeout          = "90s"
        prefix_rewrite   = "/"
      }
    }
  }
}
