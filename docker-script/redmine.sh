#!/bin/sh

. $(dirname $0)/docker-common.inc

REDMINE_MYSQL_APPLICATION_NAME="MySQL for Redmine"
REDMINE_MYSQL_IMAGE_TAG=mysql:5.6-utf8
REDMINE_MYSQL_NAME=redmine-mysql
REDMINE_MYSQL_DATA_DIR=/srv/redmine/mysql
REDMINE_MYSQL_PASSWORD=redmine-mysql-root
REDMINE_MYSQL_PARAMS=" \
    -e MYSQL_ROOT_PASSWORD=${REDMINE_MYSQL_PASSWORD} \
    -v ${REDMINE_MYSQL_DATA_DIR}:/var/lib/mysql"

REDMINE_APPLICATION_NAME=Redmine
REDMINE_IMAGE_TAG=redmine:3.2.3-smtp
REDMINE_NAME=kddi-redmine
REDMINE_DATA_DIR=/srv/redmine/data
REDMINE_REPO_DATA_DIR=/srv/redmine/repos
REDMINE_SSH_KEY_DIR=/srv/redmine/ssh-keys
REDMINE_PORT=8899
REDMINE_PARAMS=" \
    --link ${REDMINE_MYSQL_NAME}:mysql \
    -e MYSQL_ENV_MYSQL_USER=root \
    -e MYSQL_ENV_MYSQL_PASSWORD=${REDMINE_MYSQL_PASSWORD} \
    -e MYSQL_ENV_MYSQL_DATABASE=redmine \
    -p ${REDMINE_PORT}:3000 \
    -v ${REDMINE_DATA_DIR}:/usr/src/redmine/files \
    -v ${REDMINE_REPO_DATA_DIR}:/repos"

status ()
{
  #MySQL for Redmine
  Show_Running_Status "${REDMINE_MYSQL_APPLICATION_NAME}" "${REDMINE_MYSQL_NAME}"

  #Redmine
  Show_Running_Status "${REDMINE_APPLICATION_NAME}" "${REDMINE_NAME}"

  RETVAL=$?
  return $RETVAL
}

stop_mysql ()
{
  Stop_Application "${REDMINE_MYSQL_APPLICATION_NAME}" "${REDMINE_MYSQL_NAME}"
  RETVAL=$?
  return $RETVAL
}

start_mysql ()
{
  Start_Application_As_Daemon "${REDMINE_MYSQL_APPLICATION_NAME}" \
      "${REDMINE_MYSQL_IMAGE_TAG}" "${REDMINE_MYSQL_NAME}" "${REDMINE_MYSQL_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

stop_redmine ()
{
  Stop_Application "${REDMINE_APPLICATION_NAME}" "${REDMINE_NAME}"
  RETVAL=$?
  return $RETVAL
}

start_redmine ()
{
  Start_Application_As_Daemon "${REDMINE_APPLICATION_NAME}" \
      "${REDMINE_IMAGE_TAG}" "${REDMINE_NAME}" "${REDMINE_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

RETVAL=0

case "$1" in
  start)
        start_mysql
        start_redmine
        ;;
  stop)
        stop_redmine
        stop_mysql
        ;;
  restart)
        stop_redmine
        stop_mysql
        start_mysql
        if [ $? = 0 ]; then
          start_redmine
        fi
        RETVAL=$?
        ;;
  status)
        status
        RETVAL=$?
        ;;
  start-mysql)
        start_mysql
        RETVAL=$?
        ;;
  stop-mysql)
        stop_mysql
        RETVAL=$?
        ;;
  start-redmine)
        start_redmine
        RETVAL=$?
        ;;
  stop-redmine)
        stop_redmine
        RETVAL=$?
        ;;
  *)
        echo $"Usage: $0 {start|stop|start-mysql|stop-mysql|start-redmine|stop-redmine|restart|status}"
        exit 1
esac

exit $RETVAL
