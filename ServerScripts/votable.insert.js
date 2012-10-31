function insert(item, user, request) {
    delete item.count;
    request.execute();
}