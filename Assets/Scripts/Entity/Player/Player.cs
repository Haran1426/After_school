using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public Slider Hpbar;



    protected override void Die()
    {
        //GameManager.Instance.GameOver();
    }
}
