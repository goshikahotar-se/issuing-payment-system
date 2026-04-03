. "$PSScriptRoot\localstack-env.ps1"

$responseJson = aws --endpoint-url $endpoint --region $region sqs receive-message --queue-url $queueUrl --max-number-of-messages 1 --wait-time-seconds 2

$response = $responseJson | ConvertFrom-Json

if ($null -ne $response.Messages -and $response.Messages.Count -gt 0)
{
    $body = ConvertFrom-Json $response.Messages[0].Body
    $receiptHandle = $response.Messages[0].ReceiptHandle
    $messageId = $response.Messages[0].MessageId
    
    Write-Host $body.Message
    Write-Host "Found message: $messageId. Deleting..." -ForegroundColor Cyan
    
    # 4. Use that handle to delete the message
    aws --endpoint-url $endpoint --region $region sqs delete-message --queue-url $queueUrl --receipt-handle $receiptHandle
        
    Write-Host "Message deleted successfully." -ForegroundColor Green
}
else{
    Write-Host "No messages found in the queue." -ForegroundColor Yellow
}