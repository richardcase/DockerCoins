FROM microsoft/dotnet:latest

# Create the app directory
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# Copy app source
COPY project.json /usr/src/app/
COPY *.cs /usr/src/app/

# Install app dependencies
RUN dotnet restore

# Start the app
CMD [ "dotnet", "run" ]