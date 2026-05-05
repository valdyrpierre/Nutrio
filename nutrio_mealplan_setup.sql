-- ============================================================
-- Nutrio meal-plan recommendation setup
-- Paste into Supabase SQL Editor and run once.
-- ============================================================

begin;

-- 1) Foods table used by search + meal recommendations
create table if not exists public.foods (
  id bigserial primary key,
  name text not null,
  calories integer not null check (calories >= 0),
  category text not null,
  added_by uuid references auth.users(id) default null
);

-- 2) Food logs table used to learn user preferences
create table if not exists public.food_logs (
  id bigserial primary key,
  user_id uuid references auth.users(id) on delete cascade,
  food_id bigint references public.foods(id) on delete set null,
  food_name text not null,
  portion_size numeric(6,2) not null default 1.00 check (portion_size > 0),
  meal_type text not null check (meal_type in ('breakfast', 'lunch', 'dinner', 'snack')),
  total_calories integer not null check (total_calories >= 0),
  logged_at timestamptz not null default now()
);

-- 3) Helpful indexes for searching and recommendations
create index if not exists idx_foods_name on public.foods (name);
create index if not exists idx_foods_category on public.foods (category);
create index if not exists idx_food_logs_user_logged_at on public.food_logs (user_id, logged_at desc);
create index if not exists idx_food_logs_food_id on public.food_logs (food_id);

-- 4) Enable Row Level Security
alter table public.foods enable row level security;
alter table public.food_logs enable row level security;

-- 5) Policies for authenticated users
-- Safe to re-run because older policies are dropped first.
drop policy if exists "Anyone can read foods" on public.foods;
create policy "Anyone can read foods"
on public.foods
for select
using (true);

drop policy if exists "Authenticated users can insert foods" on public.foods;
create policy "Authenticated users can insert foods"
on public.foods
for insert
to authenticated
with check (auth.uid() = added_by or added_by is null);

drop policy if exists "Users can read own food logs" on public.food_logs;
create policy "Users can read own food logs"
on public.food_logs
for select
to authenticated
using (auth.uid() = user_id);

drop policy if exists "Users can insert own food logs" on public.food_logs;
create policy "Users can insert own food logs"
on public.food_logs
for insert
to authenticated
with check (auth.uid() = user_id);

-- Optional: allow updates/deletes on a user's own logs
 drop policy if exists "Users can update own food logs" on public.food_logs;
create policy "Users can update own food logs"
on public.food_logs
for update
to authenticated
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

drop policy if exists "Users can delete own food logs" on public.food_logs;
create policy "Users can delete own food logs"
on public.food_logs
for delete
to authenticated
using (auth.uid() = user_id);

commit;

-- After running this file, you can also run `foods_seed.sql`
-- to insert starter foods if your foods table is empty.
