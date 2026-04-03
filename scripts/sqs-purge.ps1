. "$PSScriptRoot\localstack-env.ps1"
aws --endpoint-url=$endpoint --region $region sqs purge-queue --queue-url $queueUrl