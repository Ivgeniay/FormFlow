FROM node:18-alpine AS build

ARG REACT_APP_GOOGLE_REDIRECT_URI
ARG REACT_APP_GOOGLE_CLIENT_ID
ENV REACT_APP_GOOGLE_REDIRECT_URI=$REACT_APP_GOOGLE_REDIRECT_URI
ENV REACT_APP_GOOGLE_CLIENT_ID=$REACT_APP_GOOGLE_CLIENT_ID

WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build

FROM nginx:alpine AS production
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]