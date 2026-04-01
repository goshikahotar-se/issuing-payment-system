#!/bin/bash
set -euo pipefail


REGION="eu-west-1"

#create topic name
TOPIC_ARN=$(awslocal sns create-topic --name authorization-events --region "$REGION" --query 'TopicArn' --output text)
echo "SNS Topic created: $TOPIC_ARN"

#create SQS Queue
QUEUE_URL=$(awslocal sqs create-queue --queue-name authorization-events-queue --region "$REGION" --query 'QueueUrl' --output text)
QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url "$QUEUE_URL" --attribute-names QueueArn --region "$REGION" --query 'Attributes.QueueArn' --output text)

echo "SQS Queue created: $QUEUE_URL"

#subscribe SQS to SNS
awslocal sns subscribe \
    --topic-arn "$TOPIC_ARN" \
    --protocol sqs \
    --notification-endpoint "$QUEUE_ARN" \
    --region "$REGION"
    
echo "Subscribed queue to topic: $TOPIC_ARN"
echo "Setup complete."