### Overview
#### Server and Client
The server starts, which starts the udp service and the http service. The udp service listens for client connections.

##### Client Connection
When a client wants to connect...

### Main Features
#### UDP Connection
The udp connection is the root communication system between clients and the server. Data is sent in the form `/command/params../../`.
#### Database
The database stores items, item blueprints, users, and agents.
#### Http Server
The http server currently only exposes a bare API for checking server status. More will be done with the http server in the future.
