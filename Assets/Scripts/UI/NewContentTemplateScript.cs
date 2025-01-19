using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewContentTemplateScript : MonoBehaviour
{
    private Structure_SO structure_SO;
    private Crop_SO crop_SO;
    private LevelUpScreenUnlock LevelUpScreenUnlock;

    [SerializeField] private Image itemImg;
    // Start is called before the first frame update
    public void Init(Structure_SO structure, LevelUpScreenUnlock screen)
    {
        this.structure_SO = structure;
        itemImg.sprite = structure.displayImage;
        // Initialize structure-specific UI here
        Debug.Log($"Initialized Structure: {structure.name}");
    }

    public void Init(Crop_SO crop, LevelUpScreenUnlock screen)
    {
        this.crop_SO = crop;
        itemImg.sprite = crop.cropImage;
        // Initialize crop-specific UI here
        Debug.Log($"Initialized Crop: {crop.name}");
    }
}
