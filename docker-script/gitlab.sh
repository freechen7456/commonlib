#!/bin/sh

. $(dirname $0)/docker-common.inc

APPLICATION_NAME=GitLab

GITLAB_IMAGE_TAG=gitlab/gitlab-ce:8.9.6-ce.0
GITLIB_NAME=kddi-gitlab
GITLAB_DATA_DIR=/srv/gitlab
GITLAB_HOSTNAME=$(ip -4 addr show | grep dynamic | sed -ne 's/ *inet \([0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\)\/.*/\1/p')
GITLAB_PORT=8888
GITLAB_PORT_HTTPS=8883
GITLAB_PORT_SSH=22
GITLAB_PARAMS=" \
    --hostname "${GITLAB_HOSTNAME}" \
    -p ${GITLAB_PORT}:80 \
    -p ${GITLAB_PORT_HTTPS}:443 \
    -p ${GITLAB_PORT_SSH}:22 \
    -v ${GITLAB_DATA_DIR}/config:/etc/gitlab \
    -v ${GITLAB_DATA_DIR}/logs:/var/log/gitlab \
    -v ${GITLAB_DATA_DIR}/data:/var/opt/gitlab"

status ()
{
  Show_Running_Status "${APPLICATION_NAME}" "${GITLIB_NAME}"
  RETVAL=$?
  return $RETVAL
}

stop()
{
  Stop_Application "${APPLICATION_NAME}" "${GITLIB_NAME}"
  RETVAL=$?
  return $RETVAL
}

start ()
{
  Start_Application_As_Daemon "${APPLICATION_NAME}" "${GITLAB_IMAGE_TAG}" "${GITLIB_NAME}" "${GITLAB_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

RETVAL=0

case "$1" in
  start)
        start
        ;;
  stop)
        stop
        ;;
  restart)
        stop
        start
        RETVAL=$?
        ;;
  status)
        status
        RETVAL=$?
        ;;
  *)
        echo $"Usage: $0 {start|stop|restart|status}"
        exit 9
esac

exit $RETVAL
