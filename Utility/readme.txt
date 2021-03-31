docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=myPass123" -p 1433:1433 --name primeHotelDb -d mcr.microsoft.com/mssql/server:latest
docker start primeHotelDb