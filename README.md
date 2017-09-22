### Overview
#### Server and Client
The server starts, which starts the udp service and the http service. The udp service listens for client connections.

##### Client Connection
When a client wants to connect, it sends the connection request signal. The server instanciates a client object and stores the client in the client manager. The client has a username and password, which is required for the server to accept the request. The credentials are checked in the database upon client connection.

##### Client Disconnection
The client disconnects in one of two ways.
  - The client sends the disconnect request signal, which the server then responds on. The server deletes all the client data locally, and sends the confirmation signal to the client.
  - The client times out. This can happen if the client looses connection in some way or simply closes the client application. The server regcognizes this event when the server does not receive a response ping from the client 2 times.
  
#### Gameplay
The gameplay is focused around a client controlling a single agent. This agent has an inventory, containing items, which persist in the database. The agent is linked to the user object of the client. The user is chosen by the username upon login on the client application.

##### Storyline
You are a new recruit on the front lines of ww2. You have to survive, move up through the ranks, and develop your skills, all while participating in a persistant online war. There is only one server, with one instance, which may be balanced in some cases by server intervention. The player can move, fight, talk, trade items, improve skills and drive vehicles. There are some NPCs in the server, which can act as various useful assets in the environment, such as civilians for simple ambiance, or sales clerks at stores, such as weapon stores, food stores, hospitals, or even vehicle workshops. The NPCs can also be simple advice givers for players, such as an entity standing in a town square preaching valuable information. The environment is dynamic, objects can spawn, move and affect the game. Objects in the environment provide various levels of protection against weapons, and can affect visibility.

##### Spawning
When the player creates a new account, they will be able to choose a country. This will determing a few aspects of their spawn. In the future, players might be able to own multiple agents, each with their own country.

Players can choose either USA, Britain, France, USSR, Germany or Italy. The spawn location will be determined by the main base of operations of the nation. This will also be the default respawn point should the player die. The player will be able to set new spawn points in some towns and villages, by visiting a bus station.

Players will spawn with almost no items. Except for some RP. They will be able to purchase items from stores are traders around the world. When they respawn after death, their skills will remain, however their inventory will be cleared, with the exception of their RP.

Players will also have a banking system, so they are able to store items.

##### Combat
Combat is a fundemental aspect of the game, as it is the ultimate goal to overpower the opponent and gain territory. The combat system is only ranged, and no melee is supported. The agent has a primary weapon item slot, which can be filled when the player clicks on a weapon in the inventory. The agent also has a primary magazine item slot, which can also be filled when the player clicks on an appropriate magazine in the inventory, which is suitable for the current weapon. This means that when the weapon is switched, the magazine slot is emptied. This also opens another point that the inventory system must be very smooth, and quick to use.

The weapon currently has a few stats that affect the outcome of the shot, such as accuracy and range, however this will most likely become more complicated in the future. The magazines are able to contain rounds, which will be displayed as the stack quantity in the inventory on that perticular item. When the round count reaches 0, the item will be destroyed from the world. This does pose the issue of an imbalance in the economy, since it should be a general rule that all items are always in circulation. More on this in the items section.

Weapons can be bought from a gun store, however they will require a specific skill level to equip. This will encourage progression through the game.

##### NPCs
There are NPCs around the world, which will act as civilians, traders and store clerks. NPCs will have a faction, which can be from any nation, a specific power, such as axis or ally, or neutral. This state will be used to determine which players can interact with the NPC. The NPC faction can also change, such as a trader whos land occupation changed. This means traders in the territory of a specific faction, will always trade items of that faction.

Players can interact with NPCs by clicking on them and choosing an option. This generic behaviour dependant on the NPC presents quite a programming problem.

NPCs are server controlled, and have a tick function. This function gets called every server tick. This is useful because it allows the entity to perform actions over time, such as move to waypoints. The tick function is slow, meaning NPCs are not very responsive, but it does allow a very large number of NPCs to exist.

 - [ ] Create NPC tick waypoint system, to allow NPCs to move correctly.

### Main Features
#### UDP Connection
The udp connection is the root communication system between clients and the server. Data is sent in the form `/command/params../../`.

##### Control Priority
The client controls all player actions. All calculations are done client side. This is because it offloads the computation power from the server and allows a much higher number of clients to perform virtually any action, without further burdening the server. In essence, it vastly improves scalability.

The server is responsible for all database actions, meaning the client sends a specific request signal, which the server then executes. This means the server has full control over the database for security reasons. The server also kicks clients who are non responsive, ensuring the server is lean and clean. The server sends location update data for all nearby entities for each client. This means that each client only gets entity data for the entities within a 1 chunk range. This means the world is infinitly scalable, without any extra burden on the client, or exponential burden on the server, but rather linear.

 - [ ] Do a 1 chunk radius entity query, instead of sending all entity data.

#### Database
The database stores items, item blueprints, users, and agents.
#### Http Server
The http server currently only exposes a bare API for checking server status. More will be done with the http server in the future.
