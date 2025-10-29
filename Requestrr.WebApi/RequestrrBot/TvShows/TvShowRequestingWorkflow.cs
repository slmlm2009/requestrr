﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Requestrr.WebApi.RequestrrBot.TvShows.SeasonsRequestWorkflows;

namespace Requestrr.WebApi.RequestrrBot.TvShows
{
    public class TvShowRequestingWorkflow
    {
        private readonly TvShowUserRequester _user;
        private readonly int _categoryId;
        private readonly ITvShowSearcher _searcher;
        private readonly ITvShowRequester _requester;
        private readonly ITvShowUserInterface _userInterface;
        private readonly ITvShowNotificationWorkflow _tvShowNotificationWorkflow;
        private readonly TvShowsSettings _settings;
        private readonly IQualityProfileProvider _qualityProfileProvider;

        public TvShowRequestingWorkflow(
            TvShowUserRequester user,
            int categoryId,
            ITvShowSearcher searcher,
            ITvShowRequester requester,
            ITvShowUserInterface userInterface,
            ITvShowNotificationWorkflow tvShowNotificationWorkflow,
            TvShowsSettings settings,
            IQualityProfileProvider qualityProfileProvider)
        {
            _user = user;
            _categoryId = categoryId;
            _searcher = searcher;
            _requester = requester;
            _userInterface = userInterface;
            _tvShowNotificationWorkflow = tvShowNotificationWorkflow;
            _settings = settings;
            _qualityProfileProvider = qualityProfileProvider;
        }

        public async Task SearchTvShowAsync(string tvShowName)
        {
            var searchedTvShows = await SearchTvShowByNameAsync(tvShowName);

            if (searchedTvShows.Any())
            {
                if (searchedTvShows.Count > 1)
                {
                    await _userInterface.ShowTvShowSelection(new TvShowRequest(_user, _categoryId), searchedTvShows);
                }
                else if (searchedTvShows.Count == 1)
                {
                    var selection = searchedTvShows.Single();
                    await HandleTvShowSelectionAsync(selection.TheTvDbId);
                }
            }
        }

        public async Task SearchTvShowAsync(int tvDbId)
        {
            try
            {
                var searchedTvShow = await _searcher.SearchTvShowAsync(new TvShowRequest(_user, _categoryId), tvDbId);
                await HandleTvShowSelectionAsync(searchedTvShow.TheTvDbId);
            }
            catch
            {
                await _userInterface.WarnNoTvShowFoundByTvDbIdAsync(tvDbId);
            }
        }

        private async Task<IReadOnlyList<SearchedTvShow>> SearchTvShowByNameAsync(string tvShowName)
        {
            IReadOnlyList<SearchedTvShow> searchedTvShows = Array.Empty<SearchedTvShow>();

            tvShowName = tvShowName.Replace(".", " ");
            searchedTvShows = await _searcher.SearchTvShowAsync(new TvShowRequest(_user, _categoryId), tvShowName);

            if (!searchedTvShows.Any())
            {
                await _userInterface.WarnNoTvShowFoundAsync(tvShowName);
            }

            return searchedTvShows;
        }

        public async Task HandleTvShowSelectionAsync(int tvDbId)
        {
            var tvShow = await GetTvShowAsync(tvDbId);

            if (!tvShow.Seasons.Any() && tvShow.HasEnded)
            {
                await _userInterface.WarnShowCannotBeRequestedAsync(tvShow);
            }
            else if (tvShow.AllSeasonsFullyRequested())
            {
                if (tvShow.Seasons.OfType<FutureTvSeasons>().Any())
                {
                    await HandleSeasonSelectionAsync(tvShow, tvShow.Seasons.OfType<FutureTvSeasons>().Single());
                }
                else
                {
                    if (tvShow.HasEnded)
                    {
                        await _userInterface.WarnShowHasEndedAsync(tvShow);
                    }
                    else
                    {
                        await _userInterface.WarnAlreadySeasonAlreadyRequestedAsync(tvShow, new FutureTvSeasons());
                    }
                }
            }
            else if (!tvShow.IsMultiSeasons() && tvShow.Seasons.OfType<NormalTvSeason>().Any())
            {
                await HandleSeasonSelectionAsync(tvShow, tvShow.Seasons.OfType<NormalTvSeason>().Single());
            }
            else
            {
                var seasons = GetAvailableTvShowSeasonsBasedOnRestrictions(tvShow);

                if (seasons.Length == 1)
                {
                    await HandleSeasonSelectionAsync(tvShow, seasons.Single());
                }
                else
                {
                    await _userInterface.DisplayMultiSeasonSelectionAsync(new TvShowRequest(_user, _categoryId), tvShow, GetAvailableTvShowSeasonsBasedOnRestrictions(tvShow));
                }
            }
        }

        private TvSeason[] GetAvailableTvShowSeasonsBasedOnRestrictions(TvShow tvShow)
        {
            if (_settings.Restrictions == TvShowsRestrictions.AllSeasons)
            {
                return tvShow.Seasons.OfType<AllTvSeasons>().ToArray();
            }
            else if (_settings.Restrictions == TvShowsRestrictions.SingleSeason)
            {
                return tvShow.Seasons.Where(x => !(x is AllTvSeasons)).ToArray();
            }
            else if (_settings.Restrictions == TvShowsRestrictions.None)
            {
                return tvShow.Seasons;
            }
            else
            {
                throw new NotImplementedException($"Tv shows restriction of type {_settings.Restrictions} has not been implemented.");
            }
        }

        public async Task HandleSeasonSelectionAsync(int tvDbId, int seasonNumber)
        {
            var tvShow = await GetTvShowAsync(tvDbId);
            var selectedSeason = tvShow.Seasons.Single(x => x.SeasonNumber == seasonNumber);

            await HandleSeasonSelectionAsync(tvShow, selectedSeason);
        }

        private async Task HandleSeasonSelectionAsync(TvShow tvShow, TvSeason selectedSeason)
        {
            switch (selectedSeason)
            {
                case FutureTvSeasons futureTvSeasons:
                    await new FutureSeasonsRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .HandleSelectionAsync(new TvShowRequest(_user, _categoryId), tvShow, futureTvSeasons);
                    break;
                case AllTvSeasons allTvSeasons:
                    await new AllSeasonsRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .HandleSelectionAsync(new TvShowRequest(_user, _categoryId), tvShow, allTvSeasons);
                    break;
                case NormalTvSeason normalTvSeason:
                    await new NormalTvSeasonRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .HandleSelectionAsync(new TvShowRequest(_user, _categoryId), tvShow, normalTvSeason);
                    break;
                default:
                    throw new Exception($"Could not handle season of type \"{selectedSeason.GetType().Name}\"");
            }
        }

        public async Task RequestSeasonSelectionAsync(int tvDbId, int seasonNumber)
        {
            var tvShow = await GetTvShowAsync(tvDbId);
            var selectedSeason = tvShow.Seasons.Single(x => x.SeasonNumber == seasonNumber);

            switch (selectedSeason)
            {
                case FutureTvSeasons futureTvSeasons:
                    await new FutureSeasonsRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .RequestAsync(new TvShowRequest(_user, _categoryId), tvShow, futureTvSeasons);
                    break;
                case AllTvSeasons allTvSeasons:
                    await new AllSeasonsRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .RequestAsync(new TvShowRequest(_user, _categoryId), tvShow, allTvSeasons);
                    break;
                case NormalTvSeason normalTvSeason:
                    await new NormalTvSeasonRequestingWorkflow(_searcher, _requester, _userInterface, _tvShowNotificationWorkflow)
                        .RequestAsync(new TvShowRequest(_user, _categoryId), tvShow, normalTvSeason);
                    break;
                default:
                    throw new Exception($"Could not handle season of type \"{selectedSeason.GetType().Name}\"");
            }
        }

        private async Task<TvShow> GetTvShowAsync(int tvDbId)
        {
            var tvShow = await _searcher.GetTvShowDetailsAsync(new TvShowRequest(_user, _categoryId), tvDbId);

            if (tvShow.IsMultiSeasons())
            {
                if (_settings.Restrictions == TvShowsRestrictions.AllSeasons || tvShow.Seasons.Length <= 24)
                {
                    tvShow.Seasons = tvShow.Seasons.Prepend(new AllTvSeasons
                    {
                        SeasonNumber = 0,
                        IsAvailable = false,
                        IsRequested = RequestedState.None,
                    }).ToArray();
                }
                else
                {
                    tvShow.Seasons = tvShow.Seasons.TakeLast(25).ToArray();
                }
            }

            return tvShow;
        }
    }
}