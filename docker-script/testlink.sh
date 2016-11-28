#!/bin/sh

. $(dirname $0)/docker-common.inc

TESTLINK_NETWORK=testlink_network

TESTLINK_MARIADB_APPLICATION_NAME="MariaDB for TestLink"
TESTLINK_MARIADB_IMAGE_TAG=bitnami/mariadb
TESTLINK_MARIADB_NAME=testlink-mariadb
TESTLINK_MARIADB_DATA_DIR=/srv/testlink/mariadb
TESTLINK_MARIADB_PASSWORD=testlink-mariadb-root
TESTLINK_MARIADB_PARAMS=" \
    --net=${TESTLINK_NETWORK} \
    -v ${TESTLINK_MARIADB_DATA_DIR}:/bitnami/mariadb"

TESTLINK_APPLICATION_NAME=TestLink
TESTLINK_IMAGE_TAG=bitnami/testlink
TESTLINK_NAME=kddi-testlink
TESTLINK_DATA_DIR=/srv/testlink/data
TESTLINK_APACHE_DIR=/srv/testlink/apache
TESTLINK_PORT=8866
TESTLINK_PORT_HTTPS=8863
TESTLINK_PARAMS=" \
    --link ${TESTLINK_MARIADB_NAME}:mariadb \
    -e TESTLINK_USERNAME=admin \
    -e TESTLINK_PASSWORD=${TESTLINK_MARIADB_PASSWORD} \
    -e TESTLINK_EMAIL=testlink@example.net \
    -e TESTLINK_SMTP_ENABLE=1 \
    -e TESTLINK_SMTP_HOST=smtp3.hpe.com \
    -e TESTLINK_SMTP_PORT=25 \
    -e TESTLINK_SMTP_USER= \
    -e TESTLINK_SMTP_PASSWORD= \
    -e TESTLINK_SMTP_CONNECTION_MODE= \
    -e MARIADB_HOST=${TESTLINK_MARIADB_NAME} \
    -e MARIADB_PORT=3306 \
    -e MARIADB_USER=root \
    -e MARIADB_PASSWORD=${TESTLINK_MARIADB_PASSWORD} \
    -p ${TESTLINK_PORT}:80 \
    -p ${TESTLINK_PORT_HTTPS}:443 \
    --net=${TESTLINK_NETWORK} \
    -v ${TESTLINK_DATA_DIR}:/bitnami/testlink \
    -v ${TESTLINK_APACHE_DIR}:/bitnami/apache"

status ()
{
  #MariaDB for TestLink
  Show_Running_Status "${TESTLINK_MARIADB_APPLICATION_NAME}" "${TESTLINK_MARIADB_NAME}"

  #TestLink
  Show_Running_Status "${TESTLINK_APPLICATION_NAME}" "${TESTLINK_NAME}"

  RETVAL=$?
  return $RETVAL
}

stop_mariadb ()
{
  Stop_Application "${TESTLINK_MARIADB_APPLICATION_NAME}" "${TESTLINK_MARIADB_NAME}"
  RETVAL=$?
  return $RETVAL
}

start_mariadb ()
{
  Start_Application_As_Daemon "${TESTLINK_MARIADB_APPLICATION_NAME}" \
      "${TESTLINK_MARIADB_IMAGE_TAG}" "${TESTLINK_MARIADB_NAME}" "${TESTLINK_MARIADB_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

stop_testlink ()
{
  Stop_Application "${TESTLINK_APPLICATION_NAME}" "${TESTLINK_NAME}"
  RETVAL=$?
  return $RETVAL
}

start_testlink ()
{
  Start_Application_As_Daemon "${TESTLINK_APPLICATION_NAME}" \
      "${TESTLINK_IMAGE_TAG}" "${TESTLINK_NAME}" "${TESTLINK_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

create_network ()
{
  docker network inspect ${TESTLINK_NETWORK} &>> /dev/null
  if [ $? = 1 ]; then
    docker network create ${TESTLINK_NETWORK} &>> ${LOG_FILE}
  fi
  return $?
}

RETVAL=0

case "$1" in
  start)
        start_mariadb
        start_testlink
        ;;
  stop)
        stop_testlink
        stop_mariadb
        ;;
  restart)
        stop_testlink
        stop_mariadb
        start_mariadb
        if [ $? = 0 ]; then
          start_testlink
        fi
        RETVAL=$?
        ;;
  status)
        status
        RETVAL=$?
        ;;
  start-mariadb)
        start_mariadb
        RETVAL=$?
        ;;
  stop-mariadb)
        stop_mariadb
        RETVAL=$?
        ;;
  start-testlink)
        start_testlink
        RETVAL=$?
        ;;
  stop-testlink)
        stop_testlink
        RETVAL=$?
        ;;
  create-network)
        create_network
        RETVAL=$?
        ;;
  *)
        echo $"Usage: $0 {start|stop|start-mariadb|stop-mariadb|start-testlink|stop-testlink|restart|status|create-network}"
        exit 1
esac

exit $RETVAL
