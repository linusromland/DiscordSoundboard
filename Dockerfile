FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY ./ ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

RUN apt-get update && apt-get install -y ffmpeg opus-tools libopus-dev

VOLUME ["/App/sounds"]

ARG DISCORD_TOKEN

ENTRYPOINT ["dotnet", "DiscordSoundboard.dll"]