FROM postman/newman:4-ubuntu
WORKDIR tests
COPY Conduit.postman_collection.json .
COPY run-api-tests.sh .
RUN chmod +x run-api-tests.sh
ENTRYPOINT ["./run-api-tests.sh"]