.PHONY: init lock files ssh

lock:
	terraform providers lock -platform=windows_amd64 -platform=darwin_amd64 -platform=linux_amd64 -platform=darwin_arm64 -platform=linux_arm64 -net-mirror=https://terraform-mirror.yandexcloud.net/

init: config.s3.tfbackend backend.secret.auto.tfvars.json key.json
	terraform init -backend-config=config.s3.tfbackend

files: config.s3.tfbackend backend.secret.auto.tfvars.json key.json

key.json:
	yc iam key create --service-account-id aje7gcc8kjjht548t71i --folder-name default --output key.json

tm.key.json:
	yc iam access-key create --service-account-id ajeaems8bh468lf6imjk --format json > tm.key.json

config.s3.tfbackend: tm.key.json
	cat tm.key.json | jq -r  '"access_key=\"\(.access_key.key_id)\"\nsecret_key=\"\(.secret)\""' > config.s3.tfbackend

backend.secret.auto.tfvars.json: tm.key.json
	cat tm.key.json | jq -r  '{backend_access_key: .access_key.key_id, backend_secret_key: .secret}' > backend.secret.auto.tfvars.json

ssh:
	yc compute ssh   --id fhmaih5740gb6t8b0hi2   --identity-file ./i-bashmaky-yc.pem   --login anton
