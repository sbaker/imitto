# Set target IP address and port
$ipAddress = "127.0.0.1"
$port = 20220

# Create a new TCP client object
$tcpClient = New-Object System.Net.Sockets.TcpClient

try {
    # Attempt to connect to the server
    Write-Host "Connecting to ${ $ipAddress:$port }..."
    $tcpClient.ReceiveTimeout = 30000
    $tcpClient.SendTimeout = 30000
    $tcpClient.Connect($ipAddress, $port)
    Write-Host "Connected!"

    # Get the network stream for sending and receiving data
    $stream = $tcpClient.GetStream()
    $reader = New-Object System.IO.StreamReader($stream)
    $writer = New-Object System.IO.StreamWriter($stream)
    $writer.AutoFlush = $true

    $message = [PSCustomObject]@{
        header = @{
            length = 100
            path = "/test/topic"
            action = "Authentication"
        }
        body = @{
            topics = @("topic1", "topic2")
        }
    }

    $jsonMessage = ConvertTo-Json -InputObject $message -Depth 3

    $writer.Write($jsonMessage)

    # Receive response from the server
    if ($tcpClient.Connected) {
        while (($line = $reader.ReadLine()) -ne $null) {
            # Process each line here
            Write-Host $line
        }
        Write-Host "Received: $response"
    }

}
catch {
    Write-Error "Error: $($_.Exception.Message)"
}
finally {
    # Close the connection
    if ($tcpClient.Connected) {
        $reader.Close()
        $writer.Close()
        $tcpClient.Close()
        Write-Host "Connection closed."
    }
}