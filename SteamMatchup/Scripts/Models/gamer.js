var GamerModel = function (result) {
	this.id = result.id;
	this.name = result.name;
	this.gameIds = result.gameIds;
	this.iconUrl = result.iconUrl;

	this.displayFriends = ko.observable(false);

	this.toggleFriends = function () {
		this.displayFriends(!this.displayFriends());
	};

	var friendModels = [];
	for (var f = 0; f < result.friends.length; f++) {
		friendModels.push(new FriendModel(result.friends[f]));
	}

	this.friends = ko.observableArray(friendModels);

	this.remove = function () {
		var gamers = rootModel.gamers();

		var withoutMe = _.without(gamers, this);

		rootModel.gamers(withoutMe);
	};

	checkForMissingGameMetadata(result.gameIds, result.gameIds.length);
};

function checkForMissingGameMetadata(newGameIds, totalGames) {
	if (!newGameIds || !(newGameIds instanceof Array) || newGameIds.length == 0) {
		console.log('No new games');
		return;
	}

	$('#status').html('Downloaded ' + (totalGames - newGameIds.length) + ' out of ' + totalGames + ' games');

	console.log('checking to see if any of the ' + newGameIds.length + ' games haven\'t been downloaded');

	var existingGames = rootModel.games();
	var missingGames = [];

	for (var x = 0; x < newGameIds.length; x++) {
		var gameId = newGameIds[x];

		var game = _.find(existingGames, function (g) { return g.id == gameId; });
		if (typeof game == 'undefined' || game.length == 0) {
			missingGames.push(gameId);
		}

		if (missingGames.length >= 10) { // grab 10 games at a time
			getMissingGames(missingGames, newGameIds.slice(x + 1), totalGames);
			return;
		}
	}
}

function getMissingGames(missingGameIds, newGameIds, totalGames) {
	if (!missingGameIds || !(missingGameIds instanceof Array) || missingGameIds.length == 0) {
		console.log('No missing games');
	}
	
	console.log('Searching for games ... ');

	var payload = '';
	for (var x = 0; x < missingGameIds.length; x++) {
		payload += 'gameIds=' + missingGameIds[x] + '&';
	}

	$.ajax({
		url: '/api/info/games',
		data: payload,
		success: function (data) {
			if (!data || !data.success || data.results.length <= 0) {
				//alert('Games do not exist'); // might just happen because the games that are left don't have store pages
				return;
			}

			var games = rootModel.games();

			for (var x = 0; x < data.results.length; x++) {
				var model = new GameModel(data.results[x]);
				games.push(model);
			}

			rootModel.games(games);

			checkForMissingGameMetadata(newGameIds, totalGames);
		}
	});
}