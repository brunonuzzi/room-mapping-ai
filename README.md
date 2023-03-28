# ROOM MAPPING IA API (README.md)

## Table of Contents

- [Overview](#overview)
- [What are embeddings](#what-are-embeddings)
- [Technologies](#technologies)
- [Installation](#installation)
- [Usage](#usage)
- [Pre-seeded Data and Sample Queries](#pre-seeded-data-and-sample-queries)
- [Debugging on Visual Studio](#debugging-on-visual-studio)
- [OpenIA API Callings Samples](#openia-api-calling-samples)
- [OpenIA API Pricing](#openia-api-pricing)

## Overview

The room-mapping-ai is a proof-of-concept (POC) web api that allows users to find the best possible matches of hotel room names.

The most critical ingredient in this application is the combination of the OpenAI embedding SDK and the PostgreSQL database with the vectors extension.

When a new hotel room name is provided, the API retrieves the embedding vector from the OpenAI API and stores it in the PostgreSQL database.

It then uses native search functionality in PostgreSQL to find potential matches based on [distance vector similarity.](https://help.openai.com/en/articles/6824809-embeddings-frequently-asked-questions)

> Both the Web Api and the Postgres database run in containers, there is no need to install anything apart from docker.

> The model used for calculated the embedding on OpenIA side is: `AdaTextEmbedding`

## What are embeddings

OpenAI’s text embeddings measure the relatedness of text strings. Embeddings are commonly used for:

Search (where results are ranked by relevance to a query string)

An embedding is a vector (list) of floating point numbers. The distance between two vectors measures their relatedness.

Small distances suggest high relatedness and large distances suggest low relatedness.

## Technologies

- [OpenAI Embeddings](https://platform.openai.com/docs/guides/embeddings/what-are-embeddings)
- [OpenAI-API-dotnet](https://github.com/OkGoDoIt/OpenAI-API-dotnet)
- [PostgreSQL with the pgvector extension](https://github.com/pgvector/pgvector)
- [Docker](https://www.docker.com/)
- C# 7.0

## Installation

### Prerequisites

- Docker

### Steps

1. Clone the repository: https://github.com/brunonuzzi/room-mapping-ai

2. Create your open ai key: https://platform.openai.com/account/api-keys

3. Navigate to the project directory: `cd room-mapping-ai`

4. Replace room-mapping-ai/appsettings.json with you own open api key.

```json
"OpenAI": {
    "ApiKey": "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"
  }
```

4. Build the Docker containers using Docker: `docker-compose build`

5. Run the Docker Compose: `docker-compose up`

6. Navigate to http://localhost:8000/swagger

> To stop and remove containers: `docker-compose down --remove-orphans`

![hippo](https://media3.giphy.com/media/aUovxH8Vf9qDu/giphy.gif)

## Usage

To use the room mapping ai API, send a POST request with the hotel room name as the request body

```bash
curl -X 'POST' \
  'http://localhost:5000/api/RoomMapping/GetMostSimilarRooms?roomName=Serenity%20Luxury' \
  -H 'accept: text/plain' \
  -d ''
```

The API will respond with a list of potential matches:

```json
[
  {
    "id": 1,
    "hotelName": "Mallorca Rocks",
    "roomName": "Serenity Suite",
    "vectorDistance": 0.06337405572419863
  },
  {
    "id": 10,
    "hotelName": "Mallorca Rocks",
    "roomName": "Serenity Double Room",
    "vectorDistance": 0.08145154924929454
  },
  {
    "id": 5,
    "hotelName": "Mallorca Rocks",
    "roomName": "Royal Executive Suite",
    "vectorDistance": 0.15970261195384094
  },
  {
    "id": 4,
    "hotelName": "Mallorca Rocks",
    "roomName": "Urban Escape Penthouse",
    "vectorDistance": 0.16005893649310143
  },
  {
    "id": 2,
    "hotelName": "Mallorca Rocks",
    "roomName": "Ocean View Deluxe",
    "vectorDistance": 0.16867594238009598
  }
]
```

## Pre-seeded Data and Sample Queries

### Creating the database connection

Open your preferred PostgreSQL client (psql, pgAdmin, or DBeaver) and create a new connection:

- Host : `localhost`
- Port : `5432`
- Username: `roommapping-user`
- Password: `roommapping-password`
- Database name: `room-mapping`

### Listing Pre-seeded data

The room-mapping-ai comes with pre-seeded data to help you get started quickly. This data includes a collection of hotel room names with their respective embedding vectors. To explore the pre-seeded data and better understand the application's capabilities, you can execute sample queries directly on the PostgreSQL database.

For instance, you can use the following query to find the top 5 most similar hotel rooms to the room id = 1 (Serenity Suite),
based on their embedding vectors:

```sql
 SELECT   Id
          ,HotelName
         ,RoomName
         ,CreatedDate
         ,'Serenity Suite' SearchedRoomName
         ,embedding <=> (select embedding from rooms where id = 1)
         as vector_distance
FROM     public.Rooms
ORDER BY vector_distance limit 5
```

> The <=> operator in the previous query mean `cosine distance`, you cand find other operators [here](https://github.com/pgvector/pgvector#vector-operators)

The database will generate the following results

| ID  | Hotel Name     | Room Name             | CreatedDate | SearchedRoomName | vector_distance     |
| --- | -------------- | --------------------- | ----------- | ---------------- | ------------------- |
| 1   | Mallorca Rocks | Serenity Suite        | 2023-03-28  | Serenity Suite   | 0                   |
| 13  | Mallorca Rocks | Serenity Luxury       | 2023-03-28  | Serenity Suite   | 0.06337405572419863 |
| 10  | Mallorca Rocks | Serenity Double Room  | 2023-03-28  | Serenity Suite   | 0.06572621072063112 |
| 5   | Mallorca Rocks | Royal Executive Suite | 2023-03-28  | Serenity Suite   | 0.12737531341984398 |
| 3   | Mallorca Rocks | Garden Terrace Room   | 2023-03-28  | Serenity Suite   | 0.1471648253749488  |

> For a better understanding of how seeding is done,you can check the [seed.sql](https://github.com/brunonuzzi/room-mapping-ai) file

## Debugging on Visual Studio

To the debug the wep api on Visual Studio , first make sure to remove all the existing containers
related to the web api in order to avoid porting conflicts.

Once the containers are removed , just open the `room-mapping-ai.sln` on Visual Studio and
launch the docker compose.

The web api should launch and the debugger will be attached.

## OpenIA API Calling Samples

Code snippet to direct call the OpenAI API and generate an embedding vector for a hotel room name

```bash
curl https://api.openai.com/v1/embeddings \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer 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" \
  -d '{
    "input": "Ocean View Deluxe",
    "model": "text-embedding-ada-002"
  }'
```

The expected result:

```json
{
  "object": "list",
  "data": [
    {
      "object": "embedding",
      "index": 0,
      "embedding": [
        0.0041769226,
        0.0067320974,
        ...
      ]
    }
  ],
  "model": "text-embedding-ada-002-v2",
  "usage": {
    "prompt_tokens": 3,
    "total_tokens": 3
  }
}
```

## OpenIA API Pricing

The room-mapping-ai API utilizes the OpenAI API for generating embedding vectors.
The price right now is $0.0004/1K tokens

An average string like "Serenity Double Room" has 3 tokens.
You can check prices over [here](https://openai.com/pricing)
