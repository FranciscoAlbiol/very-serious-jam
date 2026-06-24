using System.Collections;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    public GameObject[] symbolPrefabs;

    public float symbolHeight     = 1.2f;
    public float spinSpeed        = 8f;
    public float decelerationTime = 0.4f;

    public SlotMachine.Symbol LandedSymbol { get; private set; }
    public bool IsSpinning { get; private set; }

    private GameObject[] slots      = new GameObject[3];
    private int[]        slotSymbol = new int[3];
    private float        scrollOffset = 0f;
    private SlotMachine.Symbol targetSymbol;

    void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            int s = Random.Range(0, symbolPrefabs.Length);
            slotSymbol[i] = s;
            slots[i] = Instantiate(symbolPrefabs[s], transform);
            slots[i].transform.localPosition = LocalPos(i, 0f);
        }
    }

    private Vector3 LocalPos(int slotIndex, float offset)
        => new Vector3(0f, (1 - slotIndex) * symbolHeight + offset, 0f);

    public void StartSpin(SlotMachine.Symbol result, float stopDelay)
    {
        targetSymbol = result;
        StartCoroutine(SpinRoutine(stopDelay));
    }

    private IEnumerator SpinRoutine(float stopDelay)
    {
        IsSpinning = true;

        float elapsed = 0f;
        while (elapsed < stopDelay)
        {
            scrollOffset += spinSpeed * Time.deltaTime;
            while (scrollOffset >= symbolHeight)
            {
                scrollOffset -= symbolHeight;
                RotateUp(randomise: true);
            }
            ApplyPositions();
            elapsed += Time.deltaTime;
            yield return null;
        }

        scrollOffset = 0f;
        PlaceForStop();
        ApplyPositions();

        float decTimer = 0f;
        while (decTimer < decelerationTime)
        {
            float t = decTimer / decelerationTime;
            float visualOffset = Mathf.Lerp(symbolHeight * 0.5f, 0f, t * t);
            ApplyPositions(visualOffset);
            decTimer += Time.deltaTime;
            yield return null;
        }

        ApplyPositions(0f);

        LandedSymbol = targetSymbol;
        IsSpinning = false;
    }

    private void RotateUp(bool randomise)
    {
        GameObject tmpObj = slots[2];
        int        tmpIdx = slotSymbol[2];

        slots[2]      = slots[1];  slotSymbol[2] = slotSymbol[1];
        slots[1]      = slots[0];  slotSymbol[1] = slotSymbol[0];
        slots[0]      = tmpObj;    slotSymbol[0]  = tmpIdx;

        if (randomise)
        {
            int s = Random.Range(0, symbolPrefabs.Length);
            ForceSlotSymbol(0, s);
        }
    }

    private void PlaceForStop()
    {
        ForceSlotSymbol(1, (int)targetSymbol);
        ForceSlotSymbol(0, Random.Range(0, symbolPrefabs.Length));
        ForceSlotSymbol(2, Random.Range(0, symbolPrefabs.Length));
    }

    private void ApplyPositions(float extraOffset = 0f)
    {
        for (int i = 0; i < 3; i++)
            slots[i].transform.localPosition = LocalPos(i, scrollOffset + extraOffset);
    }

    private void ForceSlotSymbol(int slot, int symbolIndex)
    {
        if (slotSymbol[slot] == symbolIndex) return;

        Destroy(slots[slot]);
        slots[slot]      = Instantiate(symbolPrefabs[symbolIndex], transform);
        slotSymbol[slot] = symbolIndex;
    }
}
