# MicroHeadRenderer
A small C# web API to grab a skin from any Minecraft backend and render its head with its hat layer. 

Should work with any implementation of Mojang's account and auth backend, but it was designed for Drasl.  

Not very flexible and not very organized, but it gets the job done for what I needed (Head icons for a Discord bridge)

## Config
``SessionServer``: Session server to pull the Minecraft profile from. On official Mojang servers this should be ``https://sessionserver.mojang.com``

``ServicesServer``: "Services" API to use in the case that a username is provided and not a UUID. On Drasl this should be the same domain, but on i.e. official Mojang servers it should be ``https://api.minecraftservices.com``

``Port``: Port to host the MicroHeadRenderer on.

## Usage
Send a GET request to the server as such:

``http://my.microhead.server:1984/avatar/{UUID/USERNAME}.png``

Returns a 128x128 render of the player's head.
