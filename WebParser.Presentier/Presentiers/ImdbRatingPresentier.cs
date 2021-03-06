﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebParser.PresentierController;
using WebParser.App;

namespace WebParser.PresentierController
{
    public class ImdbRatingPresentier : IPresentier<MovieInfoResponseDTO>
    {
        private readonly IPresentierView _view;
        private readonly IMessageServiceUI _messageServiceUI;


        public ImdbRatingPresentier(IPresentierView view, IMessageServiceUI messageServiceUI)
        {
            _view = view;
            _messageServiceUI = messageServiceUI;
        }

    

        public void Handle(MovieInfoResponseDTO response)
        {
            //_view.ImdbFilmRating = response.ImdbRating.ToString("N2");

            _view.MovieList = response.SearchResultsList.Select(p => p.Name);


        }

        public void ShowMessage(string message) => _messageServiceUI.ShowMessage(message);
        public void ShowExclamation(string exclamation) => _messageServiceUI.ShowExclamation(exclamation);
        public void ShowError(string error) => _messageServiceUI.ShowError(error);



    }
}
