locals {

  fluentbit_time_key    = "Timestamp"
  fluentbit_time_format = "%Y-%m-%dT%H:%M:%S.%L%z"
  fluentbit_message_key = "Message"
  fluentbit_level_key   = "LogLevel"

  fluentbit_conf = <<EOF
[SERVICE]
    Flush         1
    Log_File      /var/log/fluentbit.log
    Log_Level     error
    Daemon        off
    Parsers_File  /fluent-bit/etc/parsers.conf

[FILTER]
    Name parser
    Key_Name log
    Parser docker
    Reserve_Data On
    Match *

[INPUT]
    Name              forward
    Listen            0.0.0.0
    Port              24224
    Buffer_Chunk_Size 1M
    Buffer_Max_Size   6M

[OUTPUT]
    Name            yc-logging
    Match           *
    group_id        ${yandex_logging_group.logs.id}
    message_key     ${local.fluentbit_message_key}
    level_key       ${local.fluentbit_level_key}
    default_level   INFO
    authorization   instance-service-account
  EOF

  fluentbit_parsers_conf = <<EOF
[PARSER]
    Name        docker
    Format      json
    Time_Key    ${local.fluentbit_time_key}
    Time_Format ${local.fluentbit_time_format}
    Time_Keep   On
  EOF

  fluentbit_docker_logging = {
    driver = "fluentd"
    options = {
      # Fluent Bit слушает логи на порту 24224.
      fluentd-address = "localhost:24224"
      # Теги используются для маршрутизации логов.
      tag = "app.logs"
    }
  }

  fluentbit_compose = {
    container_name = "fluentbit"
    image          = "cr.yandex/yc/fluent-bit-plugin-yandex:v1.0.3-fluent-bit-1.8.6"
    ports = [
      "24224:24224",
      "24224:24224/udp"
    ]
    restart = "always"
    environment = {
      YC_GROUP_ID = yandex_logging_group.logs.id
    }
    volumes = [
      "/etc/fluentbit/fluentbit.conf:/fluent-bit/etc/fluent-bit.conf",
      "/etc/fluentbit/parsers.conf:/fluent-bit/etc/parsers.conf"
    ]
  }

  fluentbit_cloud_config_files = [
    {
      content = local.fluentbit_conf
      path    = "/etc/fluentbit/fluentbit.conf"
    },
    {
      content = local.fluentbit_parsers_conf
      path    = "/etc/fluentbit/parsers.conf"
    }
  ]
}
