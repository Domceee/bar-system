dotnet ef migrations add [pavadinimas]
dotnet ef database update

docker compose down -v
docker compose up -d
dotnet ef database update
dotnet run

npm install
npm run dev
