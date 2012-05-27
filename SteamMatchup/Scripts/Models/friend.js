var FriendModel = function (result) {
    this.id = result.id;
    this.name = result.name;
    this.iconUrl = result.iconUrl;

    this.canSelect = ko.computed(function () {
        var self = this;
        return !_.any(rootModel.gamers(), function (g) {
            return g.id == self.id;
        });
    }, this);

    this.isAdding = ko.observable(false);

    this.select = function () {
        addGamer(this.id, this.isAdding);
    };
};
