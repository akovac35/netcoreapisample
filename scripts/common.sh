#!/bin/bash
set -euo pipefail

############################################################
# This is a script which contains common code for all other
# scripts in the project.
############################################################

# Configure colors
export TXT_RED="\e[31m"
export TXT_YELLOW="\e[0;33m"
export TXT_CLEAR="\e[0m"
export TXT_GREEN="\e[0;32m"

fn_say() {
    echo -e "${TXT_CLEAR}${1}${TXT_CLEAR}"
}

fn_say_wrn() {
    echo -e "${TXT_YELLOW}${1}${TXT_CLEAR}"
}

fn_say_err() {
    # Print to stderr
	echo -e "${TXT_RED}${1}${TXT_CLEAR}" >&2 
	exit 1
}

fn_say_success() {
    echo -e "${TXT_GREEN}${1}${TXT_CLEAR}"
}

fn_debug()
{
	echo -e "DEBUG [$( caller )] ${TXT_CLEAR}${1}${TXT_CLEAR}"
}