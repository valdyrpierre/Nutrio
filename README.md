# Nutrio ML Backend Starter

This folder adds machine learning recommendations to your Nutrio project.

## What is included

- FastAPI server with endpoints:
  - `GET /health`
  - `GET /recommendations?user_id=...&limit=5`
  - `POST /feedback`
  - `POST /weekly-selections`
  - `POST /train`
- SQLAlchemy data models for foods, feedback, and model metadata
- A training script (`train.py`) using scikit-learn logistic regression
- Model persistence with joblib

## 1) Setup

```powershell
cd ml_backend
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

Copy `.env.example` to `.env` and update values if needed.

## 2) Run API

```powershell
uvicorn app:app --reload --host 127.0.0.1 --port 8000
```

## 3) Create schema / seed foods

- For PostgreSQL/Supabase, run `sql/ml_schema.sql`.
- Seed `foods` from your existing SQL in the project (`foods_seed.sql`).

For local SQLite quick start, the API auto-creates tables on startup.

## 4) Train model

```powershell
python train.py
```

or via API:

```powershell
Invoke-RestMethod -Method Post -Uri http://127.0.0.1:8000/train
```

## 5) Example requests

```powershell
Invoke-RestMethod -Uri "http://127.0.0.1:8000/recommendations?user_id=user_1&limit=5"
```

```powershell
Invoke-RestMethod -Method Post -Uri http://127.0.0.1:8000/weekly-selections -ContentType "application/json" -Body '{"user_id":"user_1","selected_food_names":["Chicken Breast","Brown Rice","Salmon","Eggs","Avocado"]}'
```
