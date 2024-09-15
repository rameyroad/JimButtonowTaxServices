# Note: this script will not run on it's own as the user must have a valid Azure account.

export ACR_NAME=rameyroad
export IMAGE_NAME=clients/jbtax-web
export TAG=latest

az login

az acr login --name $ACR_NAME

docker build -t $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG .

docker push $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG

az acr repository list --name $ACR_NAME --output table
