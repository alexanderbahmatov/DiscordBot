#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apt-get update && apt-get install -y ffmpeg
RUN apt-get install -y libopus0 libopus-dev
RUN apt-get install -y curl
RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl
RUN chmod a+rx /usr/local/bin/youtube-dl
RUN apt-get install -y python
ENV LANG C.UTF-8
ENV LC_ALL C.UTF-8
#RUN apt install -y locales
#RUN locale-gen en_US.UTF-8
#RUN dpkg-reconfigure locales
RUN echo "installed"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DiscordBot/DiscordBot.csproj", "DiscordBot/"]
COPY ["DiscordBot.Core/DiscordBot.Core.csproj", "DiscordBot.Core/"]
COPY ["DiscordBot.Player/DiscordBot.Player.csproj", "DiscordBot.Player/"]
RUN dotnet restore "DiscordBot/DiscordBot.csproj"
COPY . .
WORKDIR "/src/DiscordBot"
RUN dotnet build "DiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordBot.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.dll"]
