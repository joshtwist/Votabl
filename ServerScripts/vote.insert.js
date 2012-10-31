function insert(item, user, request) {
    item.userId = user.userId;
    request.execute({
        success: function() {
            request.respond();
            sendNotifications(item, user);
        }
    });
}

function sendNotifications(item, user) {
    // find all users who have voted on this event
    var sql = "SELECT DISTINCT c.channelUri, c.deviceType FROM channels c " +
    "INNER JOIN votes v ON c.userId = v.userId AND v.eventId = ?"  
    mssql.query(sql, [item.eventId], {
        success: function(results) {
            var wnsChannels = results.
                filter(function(r) { return r.deviceType !== "WP8" }).
                map(function(r) { return r.channelUri; });
            push.wns.sendRaw(wnsChannels, item.eventId.toString())
            var mpnsChannels = results.
                filter(function(r) { return r.deviceType === "WP8" }).
                map(function(r) { return r.channelUri; });
            push.mpns.sendRaw(mpnsChannels, item.eventId.toString(),
            {
                success: console.log
            });
        }
    });
}