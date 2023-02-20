fn getMagic() {
    return -11;
}

-getMagic();

fn getCallback() {
    fn inner() {
        return 9;
    }

    return inner;
}

-getCallback()();

+20 * -getMagic();