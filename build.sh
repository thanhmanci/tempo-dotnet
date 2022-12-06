docker build -t thanhmanci/test:tempodotnet_v03 -f TempoDotNet/Dockerfile .
docker build -t thanhmanci/test:logservice_v03 -f LogService/Dockerfile .
docker push thanhmanci/test:tempodotnet_v03
docker push thanhmanci/test:logservice_v03
