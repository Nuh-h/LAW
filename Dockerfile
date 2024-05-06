FROM mcr.microsoft.com/dotnet/core/sdk:3.1

ARG BUILD_CONFIGURATION=Debug
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=true
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
ENV ASPNETCORE_URLS=http://+;https://+
ENV ASPNETCORE_HTTPS_PORT=8000

COPY ./ArabicLearning /app
COPY ./entrypoint.sh /app

WORKDIR /app
RUN dotnet tool install -g dotnet-ef 
RUN dotnet dev-certs clean; dotnet dev-certs https;
# RUN dotnet dev-certs https
ENV PATH $PATH:/root/.dotnet/tools

RUN ["dotnet", "restore"]
RUN ["dotnet", "build"] 
EXPOSE 80/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh