public static class BattleUtility
{
    public static void AttackEnemy(JankenPlayer player,System.Func<int,int> damageModifyer = null)
    {
        var session = JankenManager.instance.nowSession;
        var id = session.GetID(player);
        if(JankenManager.instance.nowSession.playerCount == 2)
        {
            //1,0反転
            int target = (id - 1)*-1;

            var enemy = session.GetPlayer(target);
            var damage = JankenManager.instance.settingProfile.dafaultDamage;
            
            if(damageModifyer != null)
            {
                damage = damageModifyer(damage);
            }

            enemy.ModifyHP(-damage);
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public static void AttackSelf(JankenPlayer player,System.Func<int,int> damagemod = null)
    {
        var damage = JankenManager.instance.settingProfile.defaultHP;
        if(damagemod != null)
        {
            damage = damagemod(damage);
        }

        player.ModifyHP(-damage);
    }
}