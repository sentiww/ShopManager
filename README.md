# ShopManager 

## Quickstart

This application requires multiple services to function so it's best to use Docker to run it. Below is a simplified architecture diagram of how it's set up.

![architecture_simplified drawio](https://github.com/sentiww/ShopManager/assets/30019723/7051f75a-75a1-4f2d-9eb3-7bab851d6211)

To run it, simply execute the commands listed below.

```sh
git clone https://github.com/sentiww/ShopManager
cd ShopManager
docker compose up
```

You can double check if everything started corretly by running this command.

```sh
docker ps -a | grep shopmanager-shopmanager
```
After running this command, you should see something like this. All you really care about is all of these containers report "Up *x* seconds" in the fifth column. 

![image](https://github.com/sentiww/ShopManager/assets/30019723/0143e60a-0bfd-4114-b079-c8305702de93)

By default, containers will launch at these addresses:
- Frontend : localhost:3000
- Backend : localhost:8000
- Database : localhost:5432
