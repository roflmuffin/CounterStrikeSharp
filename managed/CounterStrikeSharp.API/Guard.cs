namespace CounterStrikeSharp.API
{
    public static class Guard
    {
        public static void IsValidEntity(CEntityInstance ent)
        {
            if (!ent.IsValid)
                throw new InvalidOperationException("Entity is not valid");
        }
    }
}
