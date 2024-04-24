## Quickstart

```shell
git clone https://github.com/s27496/APBDzadanie6.git
cd APBDzadanie6
docker compose up -d
```

## Rozwiązanie - Aplikacja

http://localhost:5143/swagger/index.html

## Baza danych

Do połączenia się z bazą danych:

Adres - `localhost:1433`

Nazwa użytkownika - `sa`

Hasło - `Password!`

## Użytkowanie

**Windows - docker**
```shell
docker run --network=host --rm -it curlimages/curl --location 'http://localhost:5143/api/Warehouse' --header 'Content-Type: application/json' --data '{ \"idProduct\": 1, \"idWarehouse\": 1, \"Amount\": 125, \"createdAt\": \"2024-05-22T18:23:20.750Z\" }'
```

**Linux - curl**
```shell
curl --location 'http://localhost:5143/api/Warehouse' \
--header 'Content-Type: application/json' \
--data '{
  "idProduct": 1,
  "idWarehouse": 1,
  "Amount": 125,
  "createdAt": "2024-05-22T18:23:20.750Z"
}'
```
