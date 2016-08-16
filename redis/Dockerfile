FROM        ubuntu:14.04
RUN         apt-get update && apt-get install -y redis-server
RUN         sysctl vm.overcommit_memory=1
RUN         service redis-server restart
EXPOSE      6379
ENTRYPOINT  ["/usr/bin/redis-server"]