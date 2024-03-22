docker login -u "%elders_docker_username%" -p "%elders_docker_password%" 
docker build -f ci/Dockerfile.api -t docker.io/elders.pushnotifications.api:preview .
docker build -f ci/Dockerfile.svc -t docker.io/elders.pushnotifications.svc:preview .
docker push docker.io/elders/pushnotifications.svc:preview
docker push docker.io/elders/pushnotifications.api:preview