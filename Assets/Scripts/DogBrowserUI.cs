using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DogBrowserUI : MonoBehaviour
{
    [SerializeField] private int itemsPerPage = 10;
    [SerializeField] private DogAPIProvider apiProvider;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private TextMeshProUGUI pageStatusText;
    [SerializeField] private TextMeshProUGUI infoDisplayArea;
    [SerializeField] private ScrollRect scrollRect;

    private int currentPage = 0;
    private int totalPages = 29;

    private void Start()
    {
        if (apiProvider == null)
            apiProvider = GetComponent<DogAPIProvider>();

        if (scrollRect != null && infoDisplayArea != null)
        {
            RectTransform textRect = infoDisplayArea.GetComponent<RectTransform>();
            if (scrollRect.content == null)
            {
                scrollRect.content = textRect;
            }
            
            ContentSizeFitter fitter = infoDisplayArea.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = infoDisplayArea.gameObject.AddComponent<ContentSizeFitter>();
            }
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        nextButton.onClick.AddListener(OnNextClicked);
        prevButton.onClick.AddListener(OnPrevClicked);

        FetchCurrentPage();
    }

    private void FetchCurrentPage()
    {
        infoDisplayArea.text = "Loading...";
        UpdatePaginationUI();
        
        apiProvider.GetBreeds(currentPage, itemsPerPage, OnBreedsLoaded, OnError);
    }

    private void OnBreedsLoaded(List<DogBreed> breeds, int totalCount)
    {
        if (totalCount > 0)
        {
            totalPages = Mathf.CeilToInt((float)totalCount / itemsPerPage);
        }
        else if (breeds.Count > 0 && totalCount == -1)
        {
            if (breeds.Count == itemsPerPage)
            {
                totalPages = Mathf.Max(totalPages, currentPage + 2);
            }
            else
            {
                totalPages = currentPage + 1;
            }
        }
        else if (breeds.Count == 0 && totalCount == -1)
        {
            totalPages = currentPage;
        }

        infoDisplayArea.text = "";
        foreach (var breed in breeds)
        {
            infoDisplayArea.text += $"{breed.name}\n";
            string description = !string.IsNullOrEmpty(breed.bred_for) ? breed.bred_for : breed.breed_group;
            infoDisplayArea.text += $"Bred for: {description ?? "Unknown"}\n";
            infoDisplayArea.text += $"Lifespan: {breed.life_span ?? "Unknown"}\n";
            infoDisplayArea.text += $"Temperament: {breed.temperament ?? "Unknown"}\n\n";
        }
        
        UpdatePaginationUI();
        
        Canvas.ForceUpdateCanvases();
        
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void OnError(string error)
    {
        infoDisplayArea.text = $"<color=red>Error: {error}</color>";
    }

    private void OnNextClicked()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            FetchCurrentPage();
        }
    }

    private void OnPrevClicked()
    {
        if (currentPage > 0)
        {
            currentPage--;
            FetchCurrentPage();
        }
    }

    private void UpdatePaginationUI()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;

        int displayPage = currentPage + 1;
        int pagesLeft = Mathf.Max(0, totalPages - displayPage);
        
        pageStatusText.text = $"Page: {displayPage} / {totalPages}\nPages left: {pagesLeft}";
    }
}
