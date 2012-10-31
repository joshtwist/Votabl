function insert(item, user, request) {
    item.userId = user.userId;
    request.execute({
        success: function() {
            request.respond();
            sendNotification(item, user);
            
        }
    });
}
function sendNotification(item, user) {
    var sql = "SELECT DISTINCT c.channelUri, c.deviceType FROM channels c " +
    "INNER JOIN userCategories u ON u.userId = c.userId AND u.categoryId = ?";
    mssql.query(sql, [item.categoryId], {
        success: function(results) {
            var wnsChannels = results.
                filter(function(r) { return r.deviceType !== "WP8" }).
                map(function (r) { return r.channelUri });
            push.wns.sendToastText04(wnsChannels, {
                text1 : item.title,
                text2 : item.subtitle,
                text3 : "EVENT BUDDY"
            });
            var mpnsChannels = results.
                filter(function(r) { return r.deviceType === "WP8" }).
                map(function(r) { return r.channelUri; });
           push.mpns.sendToast(mpnsChannels, {
                text1: item.title,
                text2: item.subtitle
           }, {
               success: console.log
           });
        }
    });
}