function read(query, user, request) {
    request.execute({
        success: function(votables) {
            if (votables.length === 0)
            {
                request.respond();
                return;
            }
            var sql = "SELECT COUNT(Id) as count, votableId FROM votes WHERE eventId = ? GROUP BY votableId"; 
            // we know we only query for a single event id
            mssql.query(sql, [votables[0].eventId],{
                success: function(votes) {
                    votes.forEach(function(vote) {
                        var thisVotable = votables.filter(function(v) { return v.id == vote.votableId });
                        if (thisVotable.length > 0)
                        {
                            thisVotable[0].count = vote.count;
                        }
                    });
                    request.respond();
                },
                error: function(err) {
                    console.error(err);
                    request.respond();
                }
            }); 
        }
    });
}