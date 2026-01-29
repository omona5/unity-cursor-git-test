using UnityEngine;

/// <summary>
/// 3개의 스프라이트(자릿수)로 점수를 000~999로 표시합니다.
/// 왼쪽=백의자리, 가운데=십의자리, 오른쪽=일의자리
/// </summary>
public class ScoreDigitDisplay : MonoBehaviour
{
    [Header("Digit Sprites (0~9)")]
    public Sprite[] numberSprites = new Sprite[10];

    [Header("Digit Objects (왼쪽=백의, 가운데=십의, 오른쪽=일의)")]
    public SpriteRenderer digitHundreds;  // 백의 자리
    public SpriteRenderer digitTens;      // 십의 자리
    public SpriteRenderer digitOnes;      // 일의 자리

    [Header("Rendering")]
    [Tooltip("배경보다 크게 두면 숫자가 앞에 그려집니다. 배경이 0이면 10 이상 권장")]
    public int sortingOrder = 10;

    private void Start()
    {
        ApplySortingOrder();
    }

    private void OnValidate()
    {
        ApplySortingOrder();
    }

    private void ApplySortingOrder()
    {
        if (digitHundreds != null) digitHundreds.sortingOrder = sortingOrder;
        if (digitTens != null) digitTens.sortingOrder = sortingOrder;
        if (digitOnes != null) digitOnes.sortingOrder = sortingOrder;
    }

    public void SetScore(int score)
    {
        score = Mathf.Clamp(score, 0, 999);

        int ones = score % 10;
        int tens = (score / 10) % 10;
        int hundreds = (score / 100) % 10;

        SetDigit(digitOnes, ones);
        SetDigit(digitTens, tens);
        SetDigit(digitHundreds, hundreds);
    }

    private void SetDigit(SpriteRenderer renderer, int digit)
    {
        if (renderer == null || numberSprites == null || digit < 0 || digit >= numberSprites.Length)
            return;
        if (numberSprites[digit] != null)
            renderer.sprite = numberSprites[digit];
    }
}
