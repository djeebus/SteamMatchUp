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