if (typeof Spinner == 'function') {
    var spinner = new Spinner({
        lines: 12, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 7, // The radius of the inner circle
        color: '#FFF', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false // Whether to render a shadow
    });
    var $spinners = $('.loading');
    for (var x = 0; x < $spinners.length; x++) {
        spinner.spin($spinners[x]);
    }
}

function addGamer(gamerId, inProcessMarker) {
	inProcessMarker(true);

	$.ajax({
	    url: '/api/info/gamers',
	    data: {
	        gamerIds: gamerId
	    },
	    complete: function () {
	        inProcessMarker(false);
	    },
	    success: function (data) {
	        if (!data || !data.success || data.results.length <= 0) {
	            alert('Gamer does not exist');
	            return;
	        }

	        for (var x = 0; x < data.results.length; x++) {
	            rootModel.gamers.push(new GamerModel(data.results[x]));
	            
                var currentUsers = localStorage.getItem('users');
	            currentUsers += (currentUsers == null ? '' : ',') + gamerId;
	            localStorage.setItem('users', currentUsers);
	        }
	    }
	});
}

function updateSelectableList(source, games, mapper) {
	var currentSelectables = source();

	var selectables = _.map(games, mapper);

	selectables = _.flatten(selectables);

	selectables = _.uniq(selectables);

	selectables = _.filter(selectables, function (s) {
		return s != null && s.length && s.length > 0;
	});

	var selectables = _.map(selectables.sort(), function (f) {
		return new SelectableModel(f);
	});

	for (var x = 0; x < selectables.length; x++) {
		var newSelectable = selectables[x];
		var existing = _.find(currentSelectables, function (s) {
			return s.name == newSelectable.name;
		});

		if (existing != null) {
			newSelectable.selected(existing.selected());
		}
	}

	source(selectables);
}

function filterGamesBySelectables(source, games, target) {
	var selectedItems = _.filter(source(), function (f) {
		return f.selected();
	});

	selectedItems = _.map(selectedItems, function (f) {
		return f.name;
	});

	if (selectedItems.length > 0) {
	    games = _.filter(games, function (g) {
	        if (!g.isValid) {
	            return false;
	        }
	        var targetItems = target(g);
	        if (targetItems == null) {
	            return false;
	        }

	        var matchingItems = _.intersection(target(g), selectedItems);

	        return matchingItems.length == selectedItems.length;
	    });
	}

	return games;
};

Array.prototype.remove = function (from, to) {
	var rest = this.slice((to || from) + 1 || this.length);
	this.length = from < 0 ? this.length + from : from;
	return this.push.apply(this, rest);
};

var missingGameThreadId;
function checkForMissingGameMetadata() {
    if (missingGameThreadId) {
        // already running, escape
        return;
    }

    missingGameThreadId = setTimeout(function () {
        do {
            var gamers = rootModel.gamers();

            var games = rootModel.games();

            var gameIdsFromGamers = _.pluck(gamers, 'gameIds');
            var neededGameIds = _.flatten(gameIdsFromGamers);
            var uniqueGameIds = _.uniq(neededGameIds);
            console.log('checking ' + uniqueGameIds.length + ' games to see if any haven\'t been downloaded');

            var missingGameIds = _.filter(uniqueGameIds, function (gameId) {
                return _.all(games, function (g) {
                    return g.id != gameId;
                });
            });
            console.log('' + missingGameIds.length + ' haven\'t been downloaded');

            if (missingGameIds.length == 0) {
                console.log('No missing games');
                return;
            }

            $('#status').html('Downloading ' + missingGameIds.length + ' games');

            var sliceLength = 10;
            for (var x = 0; x < missingGameIds.length; x += sliceLength) {
                var thisCheck = missingGameIds.slice(x, x + sliceLength);

                getMissingGames(thisCheck);
            }
        } while (true);

        missingGameThreadId = null;
    });
}

function getMissingGames(missingGameIds, gamesBucket) {
    if (!missingGameIds || !(missingGameIds instanceof Array) || missingGameIds.length == 0) {
        console.log('No missing games');
    }

    console.log('Searching for games ... ');

    var payload = '';
    for (var x = 0; x < missingGameIds.length; x++) {
        payload += 'gameIds=' + missingGameIds[x] + '&';
    }

    $.ajax({
        async: false,
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
        }
    });
}