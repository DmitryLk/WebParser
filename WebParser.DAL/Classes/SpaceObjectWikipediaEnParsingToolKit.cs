﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebParser.Data
{
    class SpaceObjectWikipediaEnParsingToolKit
    {
        private readonly Uri _baseUrl = new Uri("https://en.wikipedia.org/");

     

        public async Task<HtmlDocument> GetTargetPage(HtmlDocument checkedDocument)
        {
            if (CheckPage_TargetPage(checkedDocument)) return checkedDocument;

            var searchResultsPage = await GetSearchResultsPage(checkedDocument);

            foreach (var uri in ExtractListUriByInnerTextKeywordsList(searchResultsPage, new List<string> { "moon", "planet", "asteroid", "comet" }))
            {
                var resultPage = await GetHtmlDocumentByUri(uri);
                if (CheckPage_TargetPage(resultPage)) return resultPage;
            }
            throw new Exception("Не найдено страниц о космическом объекте в списке");
        }

        public async Task<HtmlDocument> GetSearchResultsPage(HtmlDocument checkedDocument)
        {
            if (CheckPage_SearchResultsPage(checkedDocument)) return checkedDocument;
            if (!CheckPage_LinkToSearchResultsPage(checkedDocument)) throw new Exception("На странице не нашлось перехода на страницу со списком");

            var disambiguationLinkList = ExtractListUriByInnerTextKeywordsList(checkedDocument, new List<string> { "(disambiguation)" });
            return await GetHtmlDocumentByUri(disambiguationLinkList.FirstOrDefault());
        }

     

     

    

        public Uri ExtractLinkFromNode(HtmlNode nodeWithHref)
        {
            Uri uri = null;
            if (nodeWithHref.Name == "a")
                uri = new Uri(_baseUrl, nodeWithHref.Attributes["href"].Value);

            return uri;
        }

        public HtmlNodeCollection FindNodesByTagAndInnerText(HtmlDocument checkedDocument, string textContains, string tag = "*")
        {
            var nodesFound = checkedDocument.DocumentNode.SelectNodes("//" + tag + "[text()[contains(., '" + textContains + "')]]");
            return nodesFound;
        }


        public Uri GetUriSpaceObjectImage(HtmlDocument checkedDocument)
        {
            string link = checkedDocument.DocumentNode.SelectSingleNode("//td[@colspan='2']")?.SelectSingleNode(".//a")?.SelectSingleNode(".//img")?.Attributes["src"]?.Value
                ?? throw new Exception("На странице не нашлось изображения космического тела");
            return new Uri(_baseUrl, link);
        }

        //--------------------------------------------------------------------------------------

        public bool CheckPage_TargetPage(HtmlDocument checkedDocument)
        {
            int i = 0;
            var keywordsList = new List<string> { "Eccentricity", "Volume", "Mass", "Orbital", "Temperature", "Semi-major", "anomaly" };

            foreach (var keyword in keywordsList)
            {
                if (FindNodesByTagAndInnerText(checkedDocument, keyword) != null) i++;
            }
            return i >= 3;
        }

        public bool CheckPage_SearchResultsPage(HtmlDocument checkedDocument)
        {
            return FindNodesByTagAndInnerText(checkedDocument, "Disambiguation pages") != null;
        }

        public bool CheckPage_LinkToSearchResultsPage(HtmlDocument checkedDocument)
        {
            return FindNodesByTagAndInnerText(checkedDocument, "(disambiguation)") != null;
        }

        public bool CheckPage_NotFoundPage(HtmlDocument checkedDocument)
        {
            return FindNodesByTagAndInnerText(checkedDocument, "К сожалению, по вашему запросу ничего не найдено...") != null;
        }

        //===================================================================================

        public int CountTagsOnPage(HtmlDocument document, string tag)
        {
            return document.DocumentNode.SelectNodes("//" + tag).Count;
        }

        public List<string> GetAllLinksFromDocument(HtmlDocument document)
        {
            List<string> links = new List<string>();
            string hrefValue;

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                hrefValue = link.Attributes["href"].Value;
                links.Add(hrefValue);
            }

            return links;
        }

        private Boolean CheckHtmlNodeWithAttributeValueExists(HtmlDocument checkedDocument, string attribute, string valueContains)
        {
            return checkedDocument.DocumentNode.SelectSingleNode("//a[contains(@" + attribute + ", '" + valueContains + "')]") != null;
        }

        private string GetAttributeValueContainsString(HtmlDocument checkedDocument, string attribute, string valueContains)
        {
            string link = checkedDocument.DocumentNode.SelectSingleNode("//a[contains(@" + attribute + ", '" + valueContains + "')]")?.Attributes["href"].Value;
            return link;
        }

        private string GetAttributeValueContainsInnerText(HtmlDocument checkedDocument, string attribute, string valueContains)
        {
            HtmlNode nodeFound = checkedDocument.DocumentNode.SelectSingleNode("//*[text()[contains(., '" + valueContains + "')]]");
            string link = nodeFound?.Attributes["href"].Value;
            return link;
        }


    }
}


