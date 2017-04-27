# Message Delivery Saga
1. Send `SendMessageEvent` command
2. Upon arrival of `MessageDelivered` event - complete saga
3. Also schedule an attempt to manually check whether message is delivered in 10 seconds
4. In 10 seconds if event never comes - send `CheckMessageDeliveryStatusRequest`. Schedule another check in 10 seconds
5. Upone 'CheckMessagDeliverStatusResponse` complete saga
