# Step 1: Build the app
FROM node:20 AS builder

WORKDIR /app

COPY package*.json ./
RUN npm ci
COPY . .
RUN npx next build

# Step 2: Run using a lightweight server
FROM node:20

WORKDIR /app
COPY --from=builder /app ./

ENV PORT=3000
EXPOSE 3000

CMD ["npm", "start"]
