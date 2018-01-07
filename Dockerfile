FROM microsoft/aspnetcore-build
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 4500/tcp
CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
